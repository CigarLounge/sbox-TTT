#!/bin/bash

git submodule update --recursive --remote
cp -a ./ttt-models/materials ./
cp -a ./ttt-models/models ./
cp -a ./ttt-models/sounds ./