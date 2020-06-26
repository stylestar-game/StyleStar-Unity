#!/bin/sh
# NOTE: ONLY RUN THIS WHEN THE BUNDLE ON OSX IS BUILT

if [ ! -d "$StyleStar.app/Contents/Frameworks/MonoEmbedRuntime/" ] 
 then
   mkdir StyleStar.app/Contents/Frameworks/MonoEmbedRuntime/
fi

if [ ! -d "$StyleStar.app/Contents/Frameworks/MonoEmbedRuntime/osx/" ]
 then
   mkdir StyleStar.app/Contents/Frameworks/MonoEmbedRuntime/osx/
fi

cp Assets/lib/libbass.dylib StyleStar.app/Contents/Frameworks/MonoEmbedRuntime/osx/
cp Assets/lib/libbass.so StyleStar.app/Contents/Frameworks/MonoEmbedRuntime/osx/
