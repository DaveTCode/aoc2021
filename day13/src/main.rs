use regex::Regex;
use std::collections::HashSet;
use std::fmt::{Display, Formatter};
use std::fs::File;
use std::hash::Hash;
use std::io::{BufRead, BufReader};

#[derive(Eq, PartialEq, Hash, Debug)]
struct Point {
    x: u32,
    y: u32,
}

#[derive(Debug)]
enum FoldDirection {
    X,
    Y,
}

#[derive(Debug)]
struct Fold {
    dir: FoldDirection,
    val: u32,
}

#[derive(Debug)]
struct Paper {
    points: HashSet<Point>,
}

impl Paper {
    fn height(&self) -> u32 {
        self.points.iter().map(|p| p.y).max().unwrap()
    }

    fn width(&self) -> u32 {
        self.points.iter().map(|p| p.x).max().unwrap()
    }
}

impl Display for Paper {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{}",
            (0..=self.height())
                .map(|y| {
                    (0..=self.width())
                        .map(|x| {
                            let point_exists = self.points.contains(&Point { x, y });
                            match point_exists {
                                true => "#",
                                false => ".",
                            }
                        })
                        .collect::<Vec<&str>>()
                        .join(" ")
                })
                .collect::<Vec<String>>()
                .join("\n")
        )
    }
}

#[derive(Debug)]
struct Input {
    paper: Paper,
    folds: Vec<Fold>,
}

fn parse_file(path: &str) -> Input {
    let file = File::open(path).unwrap();
    let reader = BufReader::new(file);

    let fold_regex = Regex::new(r"fold along (?P<dir>[xy])=(?P<val>\d+)").unwrap();
    let lines: Vec<String> = reader.lines().map(|line| line.unwrap()).collect();
    Input {
        paper: Paper {
            points: lines
                .iter()
                .filter(|line| line.contains(','))
                .map(|line| {
                    let parts = line.split(',').collect::<Vec<&str>>();
                    Point {
                        x: parts[0].parse().unwrap(),
                        y: parts[1].parse().unwrap(),
                    }
                })
                .collect(),
        },
        folds: lines
            .iter()
            .filter(|line| line.contains('='))
            .map(|line| {
                let caps = fold_regex.captures(line).unwrap();
                Fold {
                    dir: match &caps["dir"] {
                        "x" => FoldDirection::X,
                        "y" => FoldDirection::Y,
                        _ => panic!("invalid fold direction"),
                    },
                    val: caps["val"].parse().unwrap(),
                }
            })
            .collect(),
    }
}

fn fold_paper(paper: &Paper, fold: &Fold) -> Paper {
    // Remove any points on the fold line
    let filtered_points = paper.points.iter().filter(|point| match fold.dir {
        FoldDirection::X => point.x != fold.val,
        FoldDirection::Y => point.y != fold.val,
    });

    let transformed_points = filtered_points.map(|point| match fold.dir {
        FoldDirection::X => match point.x < fold.val {
            true => Point {
                x: point.x,
                y: point.y,
            },
            false => Point {
                x: 2 * fold.val - point.x,
                y: point.y,
            },
        },
        FoldDirection::Y => match point.y < fold.val {
            true => Point {
                x: point.x,
                y: point.y,
            },
            false => Point {
                x: point.x,
                y: 2 * fold.val - point.y,
            },
        },
    });

    let deduplicated_points = HashSet::from_iter(transformed_points);
    Paper {
        points: deduplicated_points,
    }
}

fn main() {
    let test_input = parse_file("test.txt");
    println!("{:?}", test_input);
    let real_input = parse_file("input.txt");
    println!("{:?}", real_input);

    println!("Test data");
    let mut test_paper = test_input.paper;
    for fold in test_input.folds {
        test_paper = fold_paper(&test_paper, &fold);
        println!("After fold {:?}, {} points left", &fold, test_paper.points.len());
    }

    println!("Real data");
    let mut paper = real_input.paper;
    for fold in real_input.folds {
        paper = fold_paper(&paper, &fold);
        println!("After fold {:?}, {} points left", &fold, paper.points.len());
    }
    println!("{}", test_paper);
    println!("{}", paper);
}
