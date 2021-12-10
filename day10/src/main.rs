use std::fmt::{Display, Formatter};
use std::fs::File;
use std::io::{BufRead, BufReader};

#[derive(Debug)]
enum SyntaxError {
    Corrupt(String, char, i32),
    Incomplete(String, Vec<char>),
}

impl Display for SyntaxError {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        match self {
            SyntaxError::Corrupt(s, c, n) => write!(f, "Corrupt Line {}, {}, {}", s, c, n),
            SyntaxError::Incomplete(s, _) => write!(f, "Incomplete Line {}", s),
        }
    }
}

impl std::error::Error for SyntaxError {}

fn main() {
    let test = load_file("test.txt");
    let input = load_file("input.txt");

    let test_parsed = test.into_iter().map(parse_line).collect::<Vec<Result<String, SyntaxError>>>();
    let input_parsed = input.into_iter().map(parse_line).collect::<Vec<Result<String, SyntaxError>>>();

    println!("{}", part_1(&test_parsed));
    println!("{}", part_1(&input_parsed));
    println!("{}", part_2(&test_parsed));
    println!("{}", part_2(&input_parsed));
}

fn part_1(input: &[Result<String, SyntaxError>]) -> i32 {
    input.iter().map(|r| match r {
        Err(SyntaxError::Corrupt(_, _, n)) => *n,
        _ => 0
    }).sum()
}

fn part_2(input: &[Result<String, SyntaxError>]) -> i64 {
    let mut results = input.iter().map(|r| match r {
        Err(SyntaxError::Incomplete(_, missing)) => {
            missing.iter().rfold(0i64, |acc, c| {
                acc * 5 + match c {
                    '(' => 1i64,
                    '[' => 2i64,
                    '{' => 3i64,
                    '<' => 4i64,
                    _ => panic!()
                }
            })
        },
        _ => 0i64
    }).filter(|s| { *s != 0 }).collect::<Vec<i64>>();

    results.sort_unstable();
    results[results.len() / 2]
}

fn load_file(path: &str) -> Vec<String> {
    let file = File::open(path).unwrap();
    let reader = BufReader::new(file);
    reader
        .lines()
        .map(|line| { line.expect("Could not parse line") })
        .collect()
}

fn parse_line(line: String) -> Result<String, SyntaxError> {
    let mut s = Vec::new();
    for c in line.chars() {
        match c {
            '(' | '{' | '[' | '<' => s.push(c),
            ')' | '}' | ']' | '>' => {
                let check_char = s.pop().unwrap();
                match (c, check_char) {
                    (')', '(') | ('}', '{') | (']', '[') | ('>', '<') => (),
                    (')', _) => return Err(SyntaxError::Corrupt(line, c, 3)),
                    (']', _) => return Err(SyntaxError::Corrupt(line, c, 57)),
                    ('}', _) => return Err(SyntaxError::Corrupt(line, c, 1197)),
                    ('>', _) => return Err(SyntaxError::Corrupt(line, c, 25137)),
                    _ => panic!("Invalid state"),
                }
            },
            _ => panic!("Invalid char {}", c)
        }
    }

    if s.is_empty() { return Ok(line) }

    Err(SyntaxError::Incomplete(line, s))
}
