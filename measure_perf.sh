#!/usr/bin/env bash

dotnet-counters collect --process-id $PID
# Replicating the "one pager" methodology of Zhiguo Zhou et al.
# This should be part of `perfcollect`
# perf stat -x , -e task-clock,cycles,instructions,cycles:k,instructions:k,cycles:u,instructions:u -- dotnet run
