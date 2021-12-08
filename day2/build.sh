llc day2.ll # Dump assembly for debugging
llc -filetype=obj day2.ll -o day2.o
clang day2.o -o app