from typing import List


def process_file(file: str)-> List[int]:
  with open(file, 'r') as ifile:
    return [int(f) for f in ifile.readline().split(',')]


def run_for_days(days: int, fish: List[int]) -> List[int]:
  working_set = [0 for _ in range(0, 9)]
  for i in range(0, 9):
    working_set[i] = len([0 for f in fish if f == i])

  for _ in range(0, days):
    new_fish = working_set[0]
    for i in range(1, 9):
      working_set[i - 1] = working_set[i]
    working_set[8] = new_fish
    working_set[6] += new_fish

  return working_set


def run_test() -> None:
  fish = process_file('test.txt')
  print(sum(run_for_days(80, fish)))
  print(sum(run_for_days(256, fish)))


def run_real() -> None:
  fish = process_file('input.txt')
  print(sum(run_for_days(80, fish)))
  print(sum(run_for_days(256, fish)))

run_test()
run_real()