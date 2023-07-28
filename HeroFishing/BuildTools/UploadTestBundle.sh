#!/bin/bash

echo $1
if [ "$1" != "" ]
then
	sh ./UploadBundle.sh aurofortest aura_bundle_test $1
fi