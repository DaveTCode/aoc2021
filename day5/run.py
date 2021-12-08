from typing import List, Tuple


def all_points_on_h_v_line(line: Tuple[Tuple[int, int], Tuple[int, int]]) -> List[Tuple[int, int]]:
  points = []

  (start, end) = line
  x_offset = y_offset = 0

  if start[0] < end[0]:
    x_offset = 1
  if start[0] > end[0]:
    x_offset = -1
  if start[1] < end[1]:
    y_offset = 1
  if start[1] > end[1]:
    y_offset = -1

  (curr_x, curr_y) = start
  while not curr_x == end[0] + x_offset or not curr_y == end[1] + y_offset:
    points.append((curr_x, curr_y))
    curr_y += y_offset
    curr_x += x_offset

  return points


def parse_coord_pairs_from_file(file: str) -> Tuple[List[Tuple[Tuple[int, int], Tuple[int, int]]], int, int]:
  coord_pairs = []
  max_x = 0
  max_y = 0
  with open(file) as ifile:
    for line in ifile:
      parts = line.split(' -> ')
      start = parts[0].split(",")
      end = parts[1].split(",")
      coord_pairs.append(((int(start[0]), int(start[1])), (int(end[0]), int(end[1]))))
      max_x = max(max_x, int(start[0]), int(end[0]))
      max_y = max(max_y, int(start[1]), int(end[1]))
  
  return (coord_pairs, max_x, max_y)

def populate_grid_part_1(coords: List[Tuple[Tuple[int, int], Tuple[int, int]]], max_x: int, max_y: int) -> List[List[int]]:
  grid = [[0 for _ in range(0, max_x + 1)] for _ in range(0, max_y + 1)]
  for coord_pair in coords:
    if coord_pair[0][0] == coord_pair[1][0] or coord_pair[0][1] == coord_pair[1][1]:
      points = all_points_on_h_v_line(coord_pair)
      for (x, y) in points:
        grid[y][x] += 1

  return grid

def populate_grid_part_2(coords: List[Tuple[Tuple[int, int], Tuple[int, int]]], max_x: int, max_y: int) -> List[List[int]]:
  grid = [[0 for _ in range(0, max_x + 1)] for _ in range(0, max_y + 1)]
  for coord_pair in coords:
    points = all_points_on_h_v_line(coord_pair)
    for (x, y) in points:
      grid[y][x] += 1

  return grid


def print_grid(grid: List[List[int]]) -> None:
  for line in grid:
    print(line)


def run_for_input(file: str) -> None:
  (coords, max_x, max_y) = parse_coord_pairs_from_file(file)
  grid_1 = populate_grid_part_1(coords, max_x, max_y)
  grid_2 = populate_grid_part_2(coords, max_x, max_y)

  large_overlaps_1 = large_overlaps_2 = 0
  for line in grid_1:
    for x in line:
      if x > 1:
        large_overlaps_1 += 1
  for line in grid_2:
    for x in line:
      if x > 1:
        large_overlaps_2 += 1
  print(large_overlaps_1)
  print(large_overlaps_2)


run_for_input('test.txt')
run_for_input('input.txt')