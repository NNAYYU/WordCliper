namespace WOrdCliper.Core
open System.IO
open System.Text.Json
//コピーしたものを保存しておく関数
module Strage =
     let save_word(filePath:string)(items:seq<string>)=
          let options = JsonSerializerOptions(WriteIndented = true)
          let json = JsonSerializer.Serialize(items,options)
          File.WriteAllText(filePath,json)
     let loa_word (filePath: string) : string[] =
          if File.Exists(filePath) then
               try
                    let json =File.ReadAllText(filePath)
                    JsonSerializer.Deserialize<string[]>(json)
               with _ ->
                    [||]
               else
                    [||]
//コピーしたものの重複を除く関数
module  TextFilter =
     let sanitizeAndDeduplicate (newItem: string) (existingItems: seq<string>) : string[] =
          let trimmed = newItem.Trim()
          if System.String.IsNullOrWhiteSpace(trimmed) then
               existingItems |> Seq.toArray
          else
               seq { yield trimmed; yield! existingItems }
               |> Seq.distinct
               |> Seq.toArray
