#!/usr/bin/env node
import { readFileSync } from 'fs';

function parseFile(path: string): number[][] {
  const file = readFileSync(path,'utf8');

  const lines = file.split('\n');
  return lines.map(l => l.split('').map(Number));
}

function stepGrid(grid: number[][]) {
  let flashes = new Set<number>();

  // Pass 1
  for (let row = 0; row < grid.length; row++) {
    for (let col = 0; col < grid[0].length; col++) {
      grid[col][row] += 1;
    }
  }

  // Pass 2 - (lazily just redo the n^2 loops)
  let foundFlash = true;
  while (foundFlash) {
    foundFlash = false;
    for (let row = 0; row < grid.length; row++) {
      for (let col = 0; col < grid[0].length; col++) {
        if (grid[col][row] >= 10 && !flashes.has(col * 10 + row)) {
          flashes.add(col * 10 + row);
          foundFlash = true;
  
          // Increase adjacent entries, check for flashes next time around loop
          for (let dy = -1; dy <= 1; dy++) {
            for (let dx = -1; dx <= 1; dx++) {
              if (row + dy >= 0 && row + dy < grid.length && col + dx >= 0 && col + dx < grid[0].length) {
                grid[col + dx][row + dy] += 1;
              }
            }
          }
        }
      }
    }
  }

  // Pass 3 - Zero out anything that flashes
  for (let row = 0; row < grid.length; row++) {
    for (let col = 0; col < grid[0].length; col++) {
      if (grid[col][row] >= 10) {
        grid[col][row] = 0;
      }
    }
  }

  return flashes.size;
}

function runForFile(path: string) {
  const input = parseFile(path);
  let part1 = 0;
  for (let i = 0; i < 1000; i++) {
    part1 += stepGrid(input);
    console.log(`Part 1: Flashes after ${i} = ${part1}`);
    //printGrid(input);
    if (input.filter(line => line.filter(x => x == 0).length === 10).length === 10) {
      console.log(`Part 2: ${i}`);
      break;
    }
  }
}

function printGrid(grid: number[][]) {
  for (let row = 0; row < grid.length; row++) {
    console.log(`${grid[row][0]}${grid[row][1]}${grid[row][2]}${grid[row][3]}${grid[row][4]}${grid[row][5]}${grid[row][6]}${grid[row][7]}${grid[row][8]}${grid[row][9]}`)
  }
}

runForFile("test.txt");
runForFile("input.txt");