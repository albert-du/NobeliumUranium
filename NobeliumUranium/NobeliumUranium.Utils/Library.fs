namespace NobeliumUranium.Utils

open System.Text.RegularExpressions
open System
open System.Collections.Generic
open System.Linq
open System.Text
open System.Net.NetworkInformation
open System.Net

module Preprocessing =
    let FilterUserMessage text =
        text
        |> fun i -> Regex.Replace(i, "<.*?>"," ") // gets rid of mentions from the discord message
        |> fun i -> i.Trim()
module Network = 
    let FindOpenPort startingPort =
        try
            let properties = IPGlobalProperties.GetIPGlobalProperties();
            [|
                properties.GetActiveTcpConnections()
                |> Array.filter (fun n -> n.LocalEndPoint.Port >= startingPort)
                |> Array.map (fun i -> i.LocalEndPoint.Port)

                properties.GetActiveTcpListeners()
                |> Array.filter (fun n -> n.Port >= startingPort)
                |> Array.map (fun i -> i.Port)

                properties.GetActiveUdpListeners()
                |> Array.filter (fun n -> n.Port >= startingPort)
                |> Array.map (fun i -> i.Port)
            |]
            |> Array.concat
            |> Array.sort
            |> fun portArray -> ( Array.find (fun i -> portArray.Contains(i) |> not ) [|startingPort .. int <| UInt16.MaxValue|] )
        with 
            | _ -> 0