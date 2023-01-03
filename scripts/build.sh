#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $DIR/../Formula.SimpleRepo

projectFile=$(ls *.csproj)
projectName=${projectFile::-7}
version=$(sed -ne '/Version/{s/.*<Version>\(.*\)<\/Version>.*/\1/p;q;}' <<< cat "$projectFile")

function listCommands() {
    echo ""
    echo "==========================="
    echo "$projectName ($version)"
    echo "---------------------------"
    echo "Parameter - Description"
    echo "==========================="
    echo "build     - Build project"
    echo "nuget     - Publish nuget package"
    echo ""
    echo "help      - Show help"
    echo ""
}

function cleanBuild() {
    dotnet clean
    dotnet build
    dotnet publish -c Release
}

function getNugetAPIKey() {
    if [ -z ${apikey+x} ]; then 
        if [ -z ${nugetkey+x} ]; then 
            echo "What is your nuget api key found at https://www.nuget.org/account/apikeys?"
            read apikey
        else 
            apikey=$nugetkey
        fi
    fi
    echo "nuget api key = $apikey"
}

function nuget() {
    getNugetAPIKey
    cleanBuild
    cp README.md README.txt
    dotnet pack
    rm README.txt
    dotnet nuget push ./bin/Debug/$projectName.$version.nupkg -k $apikey -s https://api.nuget.org/v3/index.json
}

function help() {
    listCommands
    echo ""
    echo "---------------------------"
    echo ""
    echo "For pushing nuget, you can create an environment variable named nugetkey to your nuget api key, or else you will be prompted to enter it."
    echo ""
    echo ""
}

if [ "build" = "${1,,}" ]; then
    cleanBuild
elif [ "help" = "${1,,}" ]; then
    help
elif [ "nuget" = "${1,,}" ]; then
    nuget
else
    listCommands
fi
