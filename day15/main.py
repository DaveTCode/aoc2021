import heapq
from typing import List

TRANSITIONS = [(-1, 0), (1, 0), (0, -1), (0, 1)]


def parse_file(path: str) -> List[List[int]]:
    with open(path, "r") as ifile:
        matrix = [[int(c) for c in line.strip()] for line in ifile]
        return matrix


def solve(graph: List[List[int]], scale: int) -> int:
    width = len(graph[0])
    height = len(graph)

    def cost(gx: int, gy: int) -> int:
        c = graph[gy % height][gx % width]
        c = c + gx // width + gy // height
        c = 1 + (c - 1) % 9
        return c

    distances = {(0, 0): 0}
    queue = [(0, (0, 0))]
    while len(queue) > 0:
        total, (x, y) = heapq.heappop(queue)
        if total <= distances[(x, y)]:
            for (nx, ny) in [
                (x + dx, y + dy)
                for dx, dy in TRANSITIONS
                if 0 <= x + dx < width * scale and 0 <= y + dy < height * scale
            ]:
                distance = total + cost(nx, ny)
                if distance < distances.get((nx, ny), 1e308):
                    distances[(nx, ny)] = distance
                    heapq.heappush(queue, (distance, (nx, ny)))

    return distances[(width * scale - 1, height * scale - 1)]


if __name__ == "__main__":
    test_graph = parse_file("test.txt")
    input_graph = parse_file("input.txt")
    print(solve(test_graph, 1))
    print(solve(input_graph, 1))
    print(solve(test_graph, 5))
    print(solve(input_graph, 5))
