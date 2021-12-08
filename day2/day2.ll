%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, %struct._IO_codecvt*, %struct._IO_wide_data*, %struct._IO_FILE*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type opaque
%struct._IO_codecvt = type opaque
%struct._IO_wide_data = type opaque

@input_file = private constant [10 x i8] c"input.txt\00"
@r = private constant [2 x i8] c"r\00"
@strformat = private constant [4 x i8] c"%d\0A\00"

declare %struct._IO_FILE* @fopen(i8*, i8*)
declare i8* @fgets(i8*, i32, %struct._IO_FILE*)
declare i32 @fclose(%struct._IO_FILE*)
declare i32 @printf(i8*, ...)
declare i32 @atoi(i8*)

define i32 @main() {
  %lineno = alloca i64
  store i64 0, i64* %lineno
  %current_line = alloca i64
  store i64 0, i64* %current_line
  %lines = alloca [1000 x [11 x i8]]

  %file = call %struct._IO_FILE* @fopen(i8* getelementptr inbounds ([10 x i8], [10 x i8]* @input_file, i64 0, i64 0), i8* getelementptr inbounds ([2 x i8], [2 x i8]* @r, i64 0, i64 0))
  %file_null_check = icmp eq %struct._IO_FILE* %file, null
  br i1 %file_null_check, label %filebad, label %loadline

filebad:
  ret i32 1

loadline:
  %lineno.no = load i64, i64* %lineno
  %lineptr = getelementptr inbounds [1000 x [11 x i8]], [1000 x [11 x i8]]* %lines, i64 0, i64 %lineno.no
  %charptr = getelementptr inbounds [11 x i8], [11 x i8]* %lineptr, i64 0, i64 0
  %fgets_return = call i8* @fgets(i8* %charptr, i32 11, %struct._IO_FILE* %file)
  %fgets_null_check = icmp eq i8* %fgets_return, null
  br i1 %fgets_null_check, label %closefile, label %nextline

nextline:
  %lineno.tmp = add i64 %lineno.no, 1
  store i64 %lineno.tmp, i64* %lineno
  br label %loadline

closefile:
  %skip_file_close_result = call i32 @fclose(%struct._IO_FILE* %file)
  br label %solve

solve:
  %result.1 = call i32 @solve_1([1000 x [11 x i8]]* %lines)
  %result.2 = call i32 @solve_2([1000 x [11 x i8]]* %lines)

  call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strformat, i64 0, i64 0), i32 %result.1)
  call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strformat, i64 0, i64 0), i32 %result.2)

  ret i32 0
}

define i32 @solve_1([1000 x [11 x i8]]* %lines) {
  %depth = alloca i32
  store i32 0, i32* %depth
  %position = alloca i32
  store i32 0, i32* %position
  %ix = alloca i64
  store i64 0, i64* %ix
  br label %loop

loop:
  %ix.val = load i64, i64* %ix
  %line = getelementptr inbounds [1000 x [11 x i8]], [1000 x [11 x i8]]* %lines, i64 0, i64 %ix.val
  %char = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 0
  %char.val = load i8, i8* %char
  %is.forward = icmp eq i8 %char.val, 102
  br i1 %is.forward, label %forward, label %checkdown

forward:
  %position.val = load i32, i32* %position
  %ix.forward = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 8
  %ix.forward.val = call i32 @atoi(i8* %ix.forward)
  %position.tmp = add i32 %position.val, %ix.forward.val
  store i32 %position.tmp, i32* %position
  br label %nextindex

checkdown:
  %depth.val = load i32, i32* %depth
  %is.down = icmp eq i8 %char.val, 100
  br i1 %is.down, label %down, label %up

down:
  %ix.down = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 5
  %ix.down.val = call i32 @atoi(i8* %ix.down)
  %depth.tmp = add i32 %depth.val, %ix.down.val
  store i32 %depth.tmp, i32* %depth
  br label %nextindex

up:
  %ix.up = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 3
  %ix.up.val = call i32 @atoi(i8* %ix.up)
  %depth.tmp.2 = sub i32 %depth.val, %ix.up.val
  store i32 %depth.tmp.2, i32* %depth
  br label %nextindex

nextindex:
  %ix.tmp = add i64 %ix.val, 1
  store i64 %ix.tmp, i64* %ix
  %checkend = icmp eq i64 %ix.val, 1000
  br i1 %checkend, label %return, label %loop

return:
  %result.depth = load i32, i32* %depth
  %result.position = load i32, i32* %position
  %result.combined = mul i32 %result.depth, %result.position
  
  call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strformat, i64 0, i64 0), i32 %result.depth)
  call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strformat, i64 0, i64 0), i32 %result.position)
  ret i32 %result.combined
}

define i32 @solve_2([1000 x [11 x i8]]* %lines) {
  %aim = alloca i32
  store i32 0, i32* %aim
  %depth = alloca i32
  store i32 0, i32* %depth
  %position = alloca i32
  store i32 0, i32* %position
  %ix = alloca i64
  store i64 0, i64* %ix
  br label %loop

loop:
  %aim.val = load i32, i32* %aim
  %ix.val = load i64, i64* %ix
  %line = getelementptr inbounds [1000 x [11 x i8]], [1000 x [11 x i8]]* %lines, i64 0, i64 %ix.val
  %char = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 0
  %char.val = load i8, i8* %char
  %is.forward = icmp eq i8 %char.val, 102
  br i1 %is.forward, label %forward, label %checkdown

forward:
  %position.val = load i32, i32* %position
  %ix.forward = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 8
  %ix.forward.val = call i32 @atoi(i8* %ix.forward)
  %position.tmp = add i32 %position.val, %ix.forward.val
  store i32 %position.tmp, i32* %position
  %depth.val = load i32, i32* %depth
  %depth.inc = mul i32 %ix.forward.val, %aim.val
  %depth.tmp = add i32 %depth.val, %depth.inc
  store i32 %depth.tmp, i32* %depth
  br label %nextindex

checkdown:
  %is.down = icmp eq i8 %char.val, 100
  br i1 %is.down, label %down, label %up

down:
  %ix.down = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 5
  %ix.down.val = call i32 @atoi(i8* %ix.down)
  %aim.tmp = add i32 %aim.val, %ix.down.val
  store i32 %aim.tmp, i32* %aim
  br label %nextindex

up:
  %ix.up = getelementptr inbounds [11 x i8], [11 x i8]* %line, i64 0, i64 3
  %ix.up.val = call i32 @atoi(i8* %ix.up)
  %aim.tmp.2 = sub i32 %aim.val, %ix.up.val
  store i32 %aim.tmp.2, i32* %aim
  br label %nextindex

nextindex:
  %ix.tmp = add i64 %ix.val, 1
  store i64 %ix.tmp, i64* %ix
  %checkend = icmp eq i64 %ix.val, 1000
  br i1 %checkend, label %return, label %loop

return:
  %result.depth = load i32, i32* %depth
  %result.position = load i32, i32* %position
  %result.combined = mul i32 %result.depth, %result.position
  
  call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strformat, i64 0, i64 0), i32 %result.depth)
  call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strformat, i64 0, i64 0), i32 %result.position)
  ret i32 %result.combined
}
