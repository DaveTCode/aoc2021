from dataclasses import dataclass
from typing import List


@dataclass
class Puzzle:
    enh: List[chr]
    grid: List[List[chr]]

    def print(self):
        for y in range(len(self.grid)):
            print("".join(self.grid[y]))

        print()


def parse(path: str) -> Puzzle:
    with open(path, 'r') as ifile:
        lines = ifile.read().split('\n')

        enh = [c for c in lines[0].rstrip()]
        grid = []
        for line in lines[2::]:
            grid.append([c for c in line])

        return Puzzle(enh, grid)


def step_puzzle(puzzle: Puzzle, is_even: bool, flip_on_even: bool) -> Puzzle:
    new_grid = [['.' for _ in range(-1, len(puzzle.grid) + 1)] for _ in range(-1, len(puzzle.grid) + 1)]

    for y in range(-1, len(puzzle.grid) + 1):
        for x in range(-1, len(puzzle.grid) + 1):
            new_grid[y + 1][x + 1] = output_pixel(puzzle, x, y, is_even, flip_on_even)

    return Puzzle(puzzle.enh, new_grid)


def output_pixel(puzzle: Puzzle, x: int, y: int, is_even: bool, flip_on_even: bool) -> chr:
    indexes = [(-1, -1), (0, -1), (1, -1), (-1, 0), (0, 0), (1, 0), (-1, 1), (0, 1), (1, 1)]
    enh_ix = 0
    acc = len(indexes) - 1

    for dx, dy in indexes:
        x1 = x + dx
        y1 = y + dy

        if 0 <= x1 < len(puzzle.grid) and 0 <= y1 < len(puzzle.grid):
            if puzzle.grid[y1][x1] == '#':
                enh_ix += 2 ** acc
        elif is_even and flip_on_even:
            enh_ix += 2 ** acc
        else:
            pass

        acc -= 1

    return puzzle.enh[enh_ix]


def solve(puzzle: Puzzle, flip_on_even: bool):
    for ix in range(0, 50):
        puzzle = step_puzzle(puzzle, ix % 2 == 1, flip_on_even)
        print(f"{ix} => {sum([x.count('#') for x in puzzle.grid])}")


if __name__ == "__main__":
    test_data = parse("test.txt")
    solve(test_data, False)
    input_data = parse("input.txt")
    solve(input_data, True)
