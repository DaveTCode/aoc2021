open System.IO

type NodeClass =
    | Start
    | End
    | Lower
    | Upper

type Node =
    { Cls: NodeClass
      Name: string
      Edges: Set<string> }

type Graph = { Nodes: Map<string, Node> }

let parseFile path =
    let lines =
        File.ReadAllLines(path)
        |> Seq.map (fun line -> line.Split('-'))

    let nodes =
        lines |> Seq.collect (fun line -> line) |> Set

    { Nodes =
          nodes
          |> Seq.map
              (fun node ->
                  node,
                  { Name = node
                    Cls =
                        match node with
                        | "start" -> Start
                        | "end" -> End
                        | n when node.ToLowerInvariant() = node -> Lower
                        | n when node.ToUpperInvariant() = node -> Upper
                        | _ -> failwith $"Bad node {node}"
                    Edges =
                        lines
                        |> Seq.filter (fun line -> line |> Seq.contains node)
                        |> Seq.map
                            (fun line ->
                                match (line.[0], line.[1]) with
                                | (a, b) when a = node -> b
                                | (a, b) when b = node -> a
                                | _ -> failwith $"Bad edge {line}")
                        |> Set })
          |> Map }

let part1 graph =
    let start = graph.Nodes["start"]

    let rec part1Paths node path =
        match node with
            | n when n.Cls = End -> Seq.singleton path
            | n when path |> Seq.contains n.Name && n.Cls = Lower -> Seq.empty
            | n -> n.Edges 
                   |> Seq.except (Seq.singleton "start") 
                   |> Seq.collect (fun e -> part1Paths graph.Nodes[e] (path |> Seq.append (Seq.singleton n.Name)))

    let paths = part1Paths start (Seq.singleton start.Name)

    Seq.distinct paths

let part2 graph = 
    let start = graph.Nodes["start"]
    
    let rec part2Paths node path =
        let maxLowerCount = match path 
                                    |> Seq.filter (fun (p: string) -> (not (p.Equals("start"))) && p.ToLowerInvariant() = p)
                                    |> Seq.countBy (fun p -> p)
                                    |> Seq.sortByDescending (fun (a, b) -> b)
                                    |> Seq.tryHead with
                                  | Some(_, m) -> m
                                  | _ -> 0
        match node with
            | n when n.Cls = End -> Seq.singleton (path |> Seq.append (Seq.singleton "end"))
            | n when path |> Seq.contains n.Name && n.Cls = Lower && maxLowerCount >= 2 -> Seq.empty
            | n -> n.Edges 
                    |> Seq.except (Seq.singleton "start") 
                    |> Seq.collect (fun e -> part2Paths graph.Nodes[e] (path 
                                                                         |> Seq.append (Seq.singleton n.Name)))
    
    let paths = part2Paths start Seq.empty
    
    Seq.distinct paths

let test1Graph = parseFile "test1.txt"
let test2Graph = parseFile "test2.txt"
let test3Graph = parseFile "test3.txt"
let inputGraph = parseFile "input.txt"

printfn "%d" (part1 test1Graph |> Seq.length)
printfn "%d" (part1 test2Graph |> Seq.length)
printfn "%d" (part1 test3Graph |> Seq.length)
printfn "%d" (part1 inputGraph |> Seq.length)

let result = part2 test1Graph
result |> Seq.map (fun path -> path |> Seq.rev |> String.concat ",") |> Seq.iter (printfn "%s")
printfn "%d" (part2 test1Graph |> Seq.length)
printfn "%d" (part2 test2Graph |> Seq.length)
printfn "%d" (part2 test3Graph |> Seq.length)
printfn "%d" (part2 inputGraph |> Seq.length)