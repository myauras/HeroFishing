#!/bin/bash

echo $1
if [ "$1" != "" ]
then
	sh ./UploadBundle.sh aurofortest aura_bundle_release $1
fi