FROM ubuntu:20.04

MAINTAINER Gabe Esteves <rgesteves@gmail.com>

USER root

ENV DEBIAN_FRONTEND noninteractive

ARG GCC_VERSION

RUN apt-get update && apt-get -qy install \
    git \
    cmake \
    libfreetype6-dev \
    flex \
    bison \
    binutils-dev \
    libiberty-dev \
    libelf-dev \
    libmpc-dev \
    g++ \
    curl \
    xz-utils \
    zip \
    unzip \
    subversion \
    python3 \
    time \
    curl \
    software-properties-common \
    && rm -rf /var/lib/apt/lists/*
    
ENV CC gcc
ENV CXX g++

RUN cd /usr/src/ \
&& git clone --single-branch --branch v5.6 https://github.com/torvalds/linux.git \
&& cd linux/tools/perf \
&& make -j"$(nproc)" \
&& cp perf /usr/bin \
&& cd /usr/src \
&& rm -rf linux

RUN curl -sSL https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
RUN apt-add-repository https://packages.microsoft.com/ubuntu/$(lsb_release -sr)/prod && apt update -y
