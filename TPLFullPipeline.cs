// $(GHDOTNET)/docs/samples/snippets/csharp/VS_Snippets_Misc/tpldataflow_palindromes/cs/dataflowpalindromes.cs

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace GHConsole2;

public class TPLFullPipeline
{
    public static void Run()
    {

    // Read "The Illiad" from Project Gutenberg at http://www.gutenberg.org/cache/epub/16452/pg16452.txt

    var contentFromFile = new TransformBlock<string, string>(fname => {
    	var inputText = File.ReadAllText(fname);
	return inputText;
    });

    var createWordList = new TransformBlock<string, string[]>(text => {
      Console.WriteLine("Creating word list...");
      char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
      text = new string(tokens);
      return text.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
    });

    var filterWordList = new TransformBlock<string[], string[]>(words => {
      Console.WriteLine("Filtering word list...");
      return words.Where(w => w.Length > 3)
      	          .Distinct()
		  .ToArray();
    });

    var findReversedWords = new TransformManyBlock<string[], string>(words => {
      Console.WriteLine("Finding reversed words...");
      var wordSet = new HashSet<string>(words);
      return from word in words.AsParallel()
        let reverse = new string(word.Reverse().ToArray())
	where word != reverse && wordSet.Contains(reverse)
	select word;
    });

    var printReversedWords = new ActionBlock<string>(reversedWord => {
      Console.WriteLine("Found reversed words {0}/{1}",
            reversedWord, new string(reversedWord.Reverse().ToArray()));
    });

    // Connect the pipeline
    var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

    contentFromFile.LinkTo(createWordList, linkOptions);
    createWordList.LinkTo(filterWordList, linkOptions);
    filterWordList.LinkTo(findReversedWords, linkOptions);
    findReversedWords.LinkTo(printReversedWords, linkOptions);

    var dataDir = Path.Combine(System.AppContext.BaseDirectory, "data");
    var inputFileName = Path.Combine(dataDir, "lorem.txt");
    contentFromFile.Post(inputFileName);
    contentFromFile.Complete();
    printReversedWords.Completion.Wait();

    Console.WriteLine("Done!");
  }
    
}
