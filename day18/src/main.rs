use crate::Number::{Pair, Regular};
use itertools::Itertools;
use std::fmt::{Display, Formatter};
use std::fs::File;
use std::io::{BufRead, BufReader};

#[derive(Debug, PartialEq, Eq, Clone)]
enum Number {
    Regular(u64),
    Pair(Box<Number>, Box<Number>),
}

impl Number {
    fn add_to_leftmost(self, val: u64) -> Self {
        match self {
            Number::Regular(n) => Number::Regular(n + val),
            Number::Pair(l, r) => Number::Pair(Box::new(l.add_to_leftmost(val)), r),
        }
    }

    fn add_to_rightmost(self, val: u64) -> Self {
        match self {
            Number::Regular(n) => Number::Regular(n + val),
            Number::Pair(l, r) => Number::Pair(l, Box::new(r.add_to_rightmost(val))),
        }
    }
}

impl Display for Number {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        match self {
            Regular(x) => write!(f, "{}", x),
            Pair(l, r) => write!(f, "[{},{}]", l, r),
        }
    }
}

fn add(a: Number, b: Number) -> Number {
    Pair(Box::from(a), Box::from(b))
}

fn reduce(a: Number) -> Number {
    fn split(n: Number) -> (Number, bool) {
        match n {
            Regular(n) if n >= 10 => (
                Pair(
                    Box::from(Regular(n / 2)),
                    Box::from(Regular((n as f64 / 2f64).ceil() as u64)),
                ),
                true,
            ),
            Regular(_) => (n, false),
            Pair(l, r) => {
                let (left_split, left_was_split) = split(*l);

                match left_was_split {
                    true => (Pair(Box::new(left_split), r), true),
                    false => {
                        let (right_split, right_was_split) = split(*r);

                        (
                            Pair(Box::new(left_split), Box::from(right_split)),
                            right_was_split,
                        )
                    }
                }
            }
        }
    }

    fn explode(n: Number, depth: usize) -> (Number, Option<(Option<u64>, Option<u64>)>) {
        match n {
            Regular(_) => (n, None),
            Pair(l, r) => match (*l, *r) {
                (Regular(nl), Regular(nr)) if depth >= 4 => {
                    (Regular(0), Some((Some(nl), Some(nr))))
                }
                (l, r) => match explode(l, depth + 1) {
                    (l_reduced, Some((explode_left, explode_right))) => {
                        let r_added = if let Some(explode_right) = explode_right {
                            r.add_to_leftmost(explode_right)
                        } else {
                            r
                        };
                        (
                            Pair(Box::new(l_reduced), Box::new(r_added)),
                            Some((explode_left, None)),
                        )
                    }
                    (l_reduced, None) => match explode(r, depth + 1) {
                        (r_reduced, Some((explode_left, explode_right))) => {
                            let l_added = if let Some(explode_left) = explode_left {
                                l_reduced.add_to_rightmost(explode_left)
                            } else {
                                l_reduced
                            };
                            (
                                Pair(Box::new(l_added), Box::new(r_reduced)),
                                Some((None, explode_right)),
                            )
                        }
                        (r_reduced, None) => (Pair(Box::new(l_reduced), Box::new(r_reduced)), None),
                    },
                },
            },
        }
    }

    let mut number = a;
    loop {
        let (next_number, res) = explode(number, 0);
        number = next_number;
        if res.is_some() {
            continue;
        };
        let (next_number, res) = split(number);
        number = next_number;
        if !res {
            break;
        }
    }
    number
}

fn magnitude(n: Number) -> u64 {
    match n {
        Regular(x) => x,
        Pair(l, r) => 3 * magnitude(*l) + 2 * magnitude(*r),
    }
}

fn parse(s: &str) -> Number {
    let chars = s.chars();

    let mut track_braces = 0;
    let mut split_point = 0;
    for (ix, c) in chars.enumerate() {
        match (c, track_braces) {
            ('[', _) => track_braces += 1,
            (']', _) => track_braces -= 1,
            (',', 1) => {
                split_point = ix;
                break;
            }
            _ => (),
        }
    }

    let (left, right) = s.split_at(split_point);
    let left_trimmed = left.strip_prefix('[').unwrap();
    let right_trimmed = right.strip_prefix(',').unwrap().strip_suffix(']').unwrap();

    Pair(
        Box::from(match left_trimmed.len() {
            1 => Regular(left_trimmed.parse::<u64>().unwrap()),
            _ => parse(left_trimmed),
        }),
        Box::from(match right_trimmed.len() {
            1 => Regular(right_trimmed.parse::<u64>().unwrap()),
            _ => parse(right_trimmed),
        }),
    )
}

fn solve(path: &str) {
    let file = File::open(path).unwrap();
    let input: Vec<_> = BufReader::new(file)
        .lines()
        .map(|line| parse(&(line.unwrap())))
        .collect();

    let result1 = {
        let sum = input
            .clone()
            .into_iter()
            .reduce(|l, r| reduce(add(l, r)))
            .unwrap();
        magnitude(sum)
    };

    println!("Part 1: {}", result1);

    let result2 = {
        input
            .into_iter()
            .permutations(2)
            .map(|permutation| {
                magnitude(
                    permutation
                        .into_iter()
                        .reduce(|l, r| reduce(add(l, r)))
                        .unwrap(),
                )
            })
            .max()
            .unwrap()
    };

    println!("Part 2: {}", result2);
}

fn main() {
    solve("c:\\test.txt");
    solve("c:\\input.txt");
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_magnitude() {
        assert_eq!(magnitude(parse("[9,1]")), 29);
        assert_eq!(magnitude(parse("[1,9]")), 21);
        assert_eq!(magnitude(parse("[[9,1],[1,9]]")), 129);
        assert_eq!(magnitude(parse("[[1,2],[[3,4],5]]")), 143);
        assert_eq!(magnitude(parse("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")), 1384);
        assert_eq!(magnitude(parse("[[[[1,1],[2,2]],[3,3]],[4,4]]")), 445);
        assert_eq!(magnitude(parse("[[[[3,0],[5,3]],[4,4]],[5,5]]")), 791);
        assert_eq!(magnitude(parse("[[[[5,0],[7,4]],[5,5]],[6,6]]")), 1137);
        assert_eq!(
            magnitude(parse(
                "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
            )),
            3488
        );
    }

    #[test]
    fn test_parse() {
        let a = Pair(Box::from(Regular(1)), Box::from(Regular(1)));
        assert_eq!(parse("[1,1]"), a);
    }

    #[test]
    fn test_one() {
        let a = Pair(Box::from(Regular(1)), Box::from(Regular(1)));
        let b = Pair(Box::from(Regular(2)), Box::from(Regular(2)));
        let c = Pair(Box::from(Regular(3)), Box::from(Regular(3)));
        let d = Pair(Box::from(Regular(4)), Box::from(Regular(4)));

        assert_eq!(
            parse("[[[[1,1],[2,2]],[3,3]],[4,4]]"),
            add(add(add(a, b), c), d)
        );
    }

    #[test]
    fn test_reduce() {
        assert_eq!(reduce(parse("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]")),)
    }
}
