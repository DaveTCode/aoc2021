use std::collections::HashMap;
use std::fs::File;
use std::io::{BufRead, BufReader};

#[derive(Debug, Eq, PartialEq, Hash, Copy, Clone)]
struct Pair {
    c0: char,
    c1: char,
}

#[derive(Debug)]
struct Input {
    sequence: HashMap<Pair, u64>,
    rules: Vec<Rule>,
}

#[derive(Debug)]
struct Rule {
    pair: Pair,
    insert: char,
}

fn parse_file(path: &str) -> Input {
    let file = File::open(path).unwrap();
    let reader = BufReader::new(file);
    let lines: Vec<String> = reader.lines().map(|line| line.unwrap()).collect();
    let mut sequence_pairs = HashMap::new();
    let sequence = lines[0].chars().collect::<Vec<char>>();
    for ix in 0..(sequence.len() - 1) {
        let pair = Pair { c0: sequence[ix], c1: sequence[ix + 1] };
        *sequence_pairs.entry(pair).or_insert(0u64) += 1u64;
    }

    Input {
        sequence: sequence_pairs,
        rules: lines
            .iter()
            .filter(|line| { line.contains("->") })
            .map(|line| {
                let parts = line.split(" -> ").collect::<Vec<_>>();
                let pair = parts[0].chars().collect::<Vec<_>>();
                let insert = parts[1].chars().collect::<Vec<_>>();
                Rule {
                    pair: Pair { c0 : pair[0], c1 : pair[1] },
                    insert: insert[0],
                }
            })
            .collect()
    }
}

fn step_input(sequence_pairs: &HashMap<Pair, u64>, rules: &Vec<Rule>) -> HashMap<Pair, u64> {
    sequence_pairs
        .iter()
        .flat_map(|(pair, count)| {
            match rules.iter().filter(|r| { r.pair == *pair }).collect::<Vec<_>>().first() {
                None => vec![(*pair, *count)],
                Some(r) => vec![
                    (Pair { c0: r.pair.c0, c1: r.insert }, *count),
                    (Pair { c0: r.insert, c1: r.pair.c1 }, *count),
                ]
            }
        })
        .fold(HashMap::new(), |mut a, (pair, count)| {
            *a.entry(pair).or_insert(0) += count;
            a
        })
}

fn most_least_common(sequence: &HashMap<Pair, u64>) -> (u64, u64) {
    let mut counts = HashMap::new();
    for (pair, val) in sequence {
        *counts.entry(pair.c0).or_insert(1u64) += val;
    }

    let mut most = (' ', 0u64);
    let mut least = (' ', u64::MAX);
    for (c, v) in counts {
        if v > most.1 { most = (c, v) }
        if v < least.1 { least = (c, v) }
    }

    println!("{:?}, {:?}", most, least);
    (most.1, least.1)
}

fn run_for_file(path: &str) {
    let input = parse_file(path);
    (0..41).enumerate().fold(input.sequence, |acc, (_, it)|{
        println!("{:?}", acc);
        let result = step_input(&acc, &input.rules);

        if it == 10 || it == 40 {
            let (most_r, least_r) = most_least_common(&acc);
            println!("{}: {}", it + 1, most_r - least_r + 1);
        }

        result
    });
}

fn main() {
    run_for_file("test.txt");
    run_for_file("input.txt");
}
