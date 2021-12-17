import itertools
from typing import Tuple, List, Set

TEST_DATA = ((20, 30), (-10, -5))
INPUT_DATA = ((135, 155), (-102, -78))


def validate_route(vx: int, vy: int, target_area: Tuple[Tuple[int, int], Tuple[int, int]]) -> Tuple[bool, Tuple[int, int]]:
    x, y = 0, 0
    while y >= target_area[1][0] and x <= target_area[0][1]:  # Stop if the probe goes beyond the target area
        x += vx
        y += vy
        vx = vx - 1 if vx > 0 else vx + 1 if vx < 0 else 0
        vy -= 1

        if target_area[1][0] <= y <= target_area[1][1] and target_area[0][0] <= x <= target_area[0][1]:
            return True, (x, y)

    return False, (x, y)


def valid_xs(target: Tuple[int, int]) -> List[Tuple[int, Set[int]]]:
    valid = []
    for init_v in range(1, target[1] + 1):
        v = init_v
        x = steps = 0
        valid_steps = set()
        while x < target[1] and not v == 0:
            steps += 1
            x += v
            v = v - 1 if v > 0 else v + 1 if v < 0 else 0
            if target[1] >= x >= target[0]:
                valid_steps.add(steps)
                if v == 0:
                    valid_steps = valid_steps.union(set(range(steps + 1, 10000)))
        if valid_steps:
            valid.append((init_v, valid_steps))

    return valid


def valid_ys(target: Tuple[int, int]) -> List[Tuple[int, Set[int], int]]:
    valid = []
    for init_v in range(target[0], 1000):
        max_y = 0
        v = init_v
        y = steps = 0
        valid_steps = set()
        while y >= target[0]:
            steps += 1
            y += v
            if y > max_y:
                max_y = y
            v -= 1
            if target[0] <= y <= target[1]:
                valid_steps.add(steps)
        if valid_steps:
            valid.append((init_v, valid_steps, max_y))

    return valid


def find_answers(target_area: Tuple[Tuple[int, int], Tuple[int, int]]) -> Tuple[int, int]:
    xs = valid_xs(target_area[0])
    ys = valid_ys(target_area[1])

    max_ys = [pair[1][2] for pair in list(itertools.product(xs, ys)) if pair[0][1] & pair[1][1]]
    max_y = max(max_ys)

    return max_y, len(max_ys)


if __name__ == '__main__':
    valid_ys(TEST_DATA[1])
    print(validate_route(7, 2, TEST_DATA))  # (True, (28, -7))
    print(validate_route(6, 3, TEST_DATA))
    print(validate_route(9, 0, TEST_DATA))
    print(validate_route(17, -4, TEST_DATA))  # False

    print(find_answers(TEST_DATA))
    print(find_answers(INPUT_DATA))

