package org.davetcode.aoc2021.day9

import java.io.File

fun main() {
    val testData = processFile("c:\\code\\home\\aoc2021day9\\test.txt")
    val inputData = processFile("c:\\code\\home\\aoc2021day9\\input.txt")
    println(part1(testData))
    println(part1(inputData))
    println(part2(testData))
    println(part2(inputData))
}

fun processFile(file: String): Array<Array<Int>> {
    return File(file)
        .readLines()
        .map { line -> line.toCharArray().map { it - '0' }.toTypedArray() }
        .toTypedArray()
}

data class Point(val x: Int, val y: Int)

fun getLowPoints(data: Array<Array<Int>>): List<Point> {
    val lowPoints = mutableListOf<Point>()
    for (y in data.indices) {
        for (x in data[0].indices) {
            val dataPoint = data[y][x]
            if (x > 0 && data[y][x - 1] <= dataPoint) continue
            if (x < data[0].size - 1 && data[y][x + 1] <= dataPoint) continue
            if (y > 0 && data[y - 1][x] <= dataPoint) continue
            if (y < data.size - 1 && data[y + 1][x] <= dataPoint) continue
            lowPoints.add(Point(x, y))
        }
    }

    return lowPoints
}

fun calculateBasinSize(data: Array<Array<Int>>, startPoint: Point): Int {
    fun inner(currentPoint: Point, basinPoints: MutableSet<Point>) {
        val (x, y) = currentPoint

        // Up
        if (y > 0 && data[y][x] < data[y - 1][x] && data[y - 1][x] != 9) {
            basinPoints.add(Point(x, y - 1))
            inner(Point(currentPoint.x, currentPoint.y - 1), basinPoints)
        }

        // Down
        if (y < (data.size - 1) && data[y][x] < data[y + 1][x] && data[y + 1][x] != 9) {
            basinPoints.add(Point(x, y + 1))
            inner(Point(currentPoint.x, currentPoint.y + 1), basinPoints)
        }

        // Left
        if (x > 0 && data[y][x] < data[y][x - 1] && data[y][x - 1] != 9) {
            basinPoints.add(Point(x - 1, y))
            inner(Point(currentPoint.x - 1, currentPoint.y), basinPoints)
        }

        // Right
        if (x < (data[y].size - 1) && data[y][x] < data[y][x + 1] && data[y][x + 1] != 9) {
            basinPoints.add(Point(x + 1, y))
            inner(Point(currentPoint.x + 1, currentPoint.y), basinPoints)
        }
    }

    val basinPoints = mutableSetOf(startPoint)
    inner(startPoint, basinPoints)
    return basinPoints.size
}

fun part2(data: Array<Array<Int>>): Int {
    val lowPoints = getLowPoints(data)

    val basins = lowPoints
        .map { calculateBasinSize(data, it) }
        .sortedDescending()
    println(basins)

    // Assume that all low points are in their own basin, no saddles
    return lowPoints
        .map { calculateBasinSize(data, it) }
        .sortedDescending()
        .take(3)
        .reduce { acc, i -> acc * i }
}

fun part1(data: Array<Array<Int>>): Int {
    val lowPoints = getLowPoints(data)
    return lowPoints.sumOf { data[it.y][it.x] + 1 }
}