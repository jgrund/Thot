module Tests.Decode

open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.Testing
open Thot.Json.Decode

[<Global>]
let it (msg: string) (f: unit->unit): unit = jsNative


[<Global>]
let describe (msg: string) (f: unit->unit): unit = jsNative

type Record2 =
    { a : float
      b : float }

    static member Create a b =
        { a = a
          b = b }

type Record3 =
    { a : float
      b : float
      c : float }

    static member Create a b c =
        { a = a
          b = b
          c = c }

type Record4 =
    { a : float
      b : float
      c : float
      d : float }

    static member Create a b c d =
        { a = a
          b = b
          c = c
          d = d }

type Record5 =
    { a : float
      b : float
      c : float
      d : float
      e : float }

    static member Create a b c d e =
        { a = a
          b = b
          c = c
          d = d
          e = e }

type Record6 =
    { a : float
      b : float
      c : float
      d : float
      e : float
      f : float }

    static member Create a b c d e f =
        { a = a
          b = b
          c = c
          d = d
          e = e
          f = f }

type Record7 =
    { a : float
      b : float
      c : float
      d : float
      e : float
      f : float
      g : float }

    static member Create a b c d e f g =
        { a = a
          b = b
          c = c
          d = d
          e = e
          f = f
          g = g }

type Record8 =
    { a : float
      b : float
      c : float
      d : float
      e : float
      f : float
      g : float
      h : float }

    static member Create a b c d e f g h =
        { a = a
          b = b
          c = c
          d = d
          e = e
          f = f
          g = g
          h = h }

let jsonRecord =
    """{ "a": 1,
         "b": 2,
         "c": 3,
         "d": 4,
         "e": 5,
         "f": 6,
         "g": 7,
         "h": 8 }"""

describe "Decode" <| fun _ ->

    describe "Primitives: " <| fun _ ->

        it "a string works" <| fun _ ->
            let expected = Ok("maxime")
            let actual =
                decodeString string "\"maxime\""

            Assert.AreEqual(expected, actual)

        it "a float works" <| fun _ ->
            let expected = Ok(1.2)
            let actual =
                decodeString float "1.2"

            Assert.AreEqual(expected, actual)

        it "null works" <| fun _ ->
            let expected = Ok(20 :> obj)
            let actual =
                decodeString (nil 20) "null"

            Assert.AreEqual(expected, actual)

        it "null works" <| fun _ ->
            let expected = Ok(false :> obj)
            let actual =
                decodeString (nil false) "null"

            Assert.AreEqual(expected, actual)

        it "a bool works" <| fun _ ->
            let expected = Ok(true)
            let actual =
                decodeString bool "true"

            Assert.AreEqual(expected, actual)

        it "an int works" <| fun _ ->
            let expected = Ok(25)
            let actual =
                decodeString int "25"

            Assert.AreEqual(expected, actual)

        it "an invalid int [invalid range] output an error (too low)" <| fun _ ->
            let expected = Error("Expecting an int but instead got:\n2147483648. Reason: Invalid range")
            let actual =
                decodeString int "2147483648"

            Assert.AreEqual(expected, actual)

        it "an invalid int [invalid range] output an error (too higth)" <| fun _ ->
            let expected = Error("Expecting an int but instead got:\n-2147483648. Reason: Invalid range")
            let actual =
                decodeString int "-2147483648"

            Assert.AreEqual(expected, actual)

    describe "Data structure" <| fun _ ->

        it "list works" <| fun _ ->
            let expected = Ok([1; 2; 3])

            let actual =
                decodeString (list int) "[1, 2, 3]"

            Assert.AreEqual(expected, actual)

        it "an invalid list output an error" <| fun _ ->
            let expected = Error("Expecting a list but instead got:\n1")

            let actual =
                decodeString (list int) "1"

            Assert.AreEqual(expected, actual)

        it "array works" <| fun _ ->
            // Need to pass by a list otherwise Fable use:
            // new Int32Array([1, 2, 3]) and the test fails
            // And this would give:
            // Expected: Result { tag: 0, data: Int32Array [ 1, 2, 3 ] }
            // Actual: Result { tag: 0, data: [ 1, 2, 3 ] }
            let expected = Ok([1; 2; 3] |> List.toArray)

            let actual =
                decodeString (array int) "[1, 2, 3]"

            Assert.AreEqual(expected, actual)

        it "an invalid array output an error" <| fun _ ->
            let expected = Error("Expecting an array but instead got:\n1")

            let actual =
                decodeString (array int) "1"

            Assert.AreEqual(expected, actual)

    describe "Inconsistent structure" <| fun _ ->

        it "optional works" <| fun _ ->
            let json = """{ "name": "maxime", "age": 25 }"""

            let expectedValid = Ok(Some "maxime")
            let actualValid =
                decodeString (optional (field "name" string) ) json

            Assert.AreEqual(expectedValid, actualValid)

            let expectedInvalidType = Ok(None)
            let actualInvalidType =
                decodeString (optional (field "name" int) ) json

            Assert.AreEqual(expectedInvalidType, actualInvalidType)

            let expectedMissingField = Ok(None)
            let actualMissingField =
                decodeString (optional (field "height" int) ) json

            Assert.AreEqual(expectedMissingField, actualMissingField)

            let expectedFieldAndThenOptional =
                Error(
                    """
Expecting an object with a field named `height` but instead got:
{
    "name": "maxime",
    "age": 25
}
                    """.Trim())
            let actualFieldAndThenOptional =
                decodeString (field "height" (optional float)) json

            Assert.AreEqual(expectedFieldAndThenOptional, actualFieldAndThenOptional)

    describe "Object primitives" <| fun _ ->

        it "at works" <| fun _ ->
            let expected = Ok("maxime")

            let actual =
                decodeString
                    (at [ "user"; "name" ] string)
                    """{ "user" : { "name": "maxime", "age": 25 } }"""

            Assert.AreEqual(expected, actual)

        it "at output an error if the paths is broken" <| fun _ ->
            let expected =
                Error(
                    """
Expecting an object with path `user.maxime` but instead got:
{
    "user": {
        "name": "maxime",
        "age": 25
    }
}
Node `maxime` is unkown.
                    """.Trim())

            let actual =
                decodeString
                    (at [ "user"; "maxime" ] string)
                    """{ "user" : { "name": "maxime", "age": 25 } }"""

            Assert.AreEqual(expected, actual)

    describe "Mapping" <| fun _ ->

        it "map works" <| fun _ ->
            let expected = Ok(6)
            let stringLength =
                map String.length string

            let actual =
                decodeString stringLength "\"maxime\""
            Assert.AreEqual(expected, actual)

        it "map2 works" <| fun _ ->
            let expected = Ok({a = 1.; b = 2.} : Record2)

            let decodePoint =
                map2 Record2.Create
                    (field "a" float)
                    (field "b" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)

        it "map3 works" <| fun _ ->
            let expected = Ok({ a = 1.
                                b = 2.
                                c = 3. } : Record3)

            let decodePoint =
                map3 Record3.Create
                    (field "a" float)
                    (field "b" float)
                    (field "c" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)

        it "map4 works" <| fun _ ->
            let expected = Ok({ a = 1.
                                b = 2.
                                c = 3.
                                d = 4. } : Record4)

            let decodePoint =
                map4 Record4.Create
                    (field "a" float)
                    (field "b" float)
                    (field "c" float)
                    (field "d" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)

        it "map5 works" <| fun _ ->
            let expected = Ok({ a = 1.
                                b = 2.
                                c = 3.
                                d = 4.
                                e = 5. } : Record5)

            let decodePoint =
                map5 Record5.Create
                    (field "a" float)
                    (field "b" float)
                    (field "c" float)
                    (field "d" float)
                    (field "e" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)

        it "map6 works" <| fun _ ->
            let expected = Ok({ a = 1.
                                b = 2.
                                c = 3.
                                d = 4.
                                e = 5.
                                f = 6. } : Record6)

            let decodePoint =
                map6 Record6.Create
                    (field "a" float)
                    (field "b" float)
                    (field "c" float)
                    (field "d" float)
                    (field "e" float)
                    (field "f" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)

        it "map7 works" <| fun _ ->
            let expected = Ok({ a = 1.
                                b = 2.
                                c = 3.
                                d = 4.
                                e = 5.
                                f = 6.
                                g = 7. } : Record7)

            let decodePoint =
                map7 Record7.Create
                    (field "a" float)
                    (field "b" float)
                    (field "c" float)
                    (field "d" float)
                    (field "e" float)
                    (field "f" float)
                    (field "g" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)

        it "map8 works" <| fun _ ->
            let expected = Ok({ a = 1.
                                b = 2.
                                c = 3.
                                d = 4.
                                e = 5.
                                f = 6.
                                g = 7.
                                h = 8. } : Record8)

            let decodePoint =
                map8 Record8.Create
                    (field "a" float)
                    (field "b" float)
                    (field "c" float)
                    (field "d" float)
                    (field "e" float)
                    (field "f" float)
                    (field "g" float)
                    (field "h" float)

            let actual =
                decodeString decodePoint jsonRecord

            Assert.AreEqual(expected, actual)
