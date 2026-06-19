# Resize
## Information
Hi, EdEnStonne here. This is a commision from Qski, not much else to add.
It allows to resize slugcats's body and tail, enjoy !

## Configuring the JSON

### Attribute description

**SlugcatName :**
The SlugcatName is the string that representing the slugcat class name (for example : "Artificer"). It is not case-specific. This attribute has one special value : `all`, which will malke the section affect all slugcats.

**ResizeAttribute :**
- `height` : accepts a float that'll modify the slugcat's height. Default is 17.
- `heightRatio` : accepts a float that'll modify the slugcat's height ratio compared to vanilla. Default is x1.
- `pupHeight` : accepts a float that'll modify the slugcat's height as a pup. Not affected by `alwaysRenderAsPup`. Default is 12.
- `pupheightRatio` : accepts a float that'll modify the slugcat's height ratio as a pup compared to vanilla. Default is x1.
- `onlyLocal` : accepts a bool that'll limit the section to only local player (that are not online Meadow players). Default is false.
- `onlyNPC` : accepts a bool that'll limit the section to only NPC slugcats. Default is false.
- `alwaysRenderAsPup` : accepts a bool that'll toggle pup render for this slugcat no matter what. Default is false.
- `tailRadiusRatio` : accepts a float that'll modify the tail thickness ratio compared to vanilla. Default is x1.
- `tailConnectionRadiusRatio` : accepts a float that'll modify the tail lenght ratio compared to vanilla. Default is x1.
- `tail` : an attribute to modify part of the tail individually. Accepts a list of `TailSegmentAttribute`, which contains multiple `TailAttribute` and their values.

**TailAttribute :**
- `index` : accept an int between 0 to 3, 0 being at the base of the tail and 3 being at the tip of it. It is mandatory for a `TailSegmentAttribute` to have this `TailAttribute`.
- `radiusRatio` : accepts a float that'll modify the tail thickness ratio of this segment. Default is x1.
- `radius` : accepts a float that'll modify the tail thickness value of this segment. Overrides `radiusRatio`. The default vanilla value depends on the segment and the slugcat (for most slugcats, the default values are [6, 4, 2.5, 1]).
- `connectionRadiusRatio` : accepts a float that'll modify the tail lenght ratio of this segment. Default is x1.
- `connectionRadius` : accepts a float that'll modify the tail lenght value of this segment. Overrides `connectionRadiusRatio`. The default vanilla value depends on the segment and the slugcat (for most slugcats, the default values are [4, 7, 7, 7]).

### Format

**Format of the whole json :**
```json
{
    SlugcatName : {
        ResizeAttribute : value,
        ResizeAttribute : value,
        ResizeAttribute : value,
        ResizeAttribute : value,
        ResizeAttribute : value,
        ...
    },
    SlugcatName : {
        ResizeAttribute : value,
        ResizeAttribute : value,
        ResizeAttribute : value,
        ResizeAttribute : value,
        ResizeAttribute : value,
        ...
    },
    ...
}
```
All `ResizeAttribute` are optional. 
If a slugcat can be asserted to multiple sections, it'll take the attribute of the first one from the top.

**Format of the "tail" ResizeAttribute :**
```json
"tail" : [
    {"index": int, TailAttribute : value, TailAttribute : value, ...}, 
    {"index": int, TailAttribute : value, TailAttribute : value, ...}, 
    {"index": int, TailAttribute : value, TailAttribute : value, ...},
    ...
]
```
The index is NOT optional, all others TailAttribute are.

### Example

```json
{
    "Survivor" : 
    {
        "height" : 19.0,
        "pupHeight" : 12.0,
        "onlyLocal" : false,
        "onlyNPC" : false,
        "tail" : [
            {"index": 0, "radius" : 8.0, "connectionRadius": 1.0}, 
            {"index": 1, "radiusRatio" : 1.1, "connectionRadiusRatio" : 1.1}, 
            {"index": 2, "radius" : 2}
        ]
    },
    "Monk" : 
    {
        "heightRatio" : 0.8,
        "pupheightRatio" : 1.0,
        "tailRadiusRatio" : 0.8,
        "tailConnectionRadiusRatio" : 0.9
    },
    "all" : 
    {
        "height" : 14.0,
        "alwaysRenderAsPup" : true
    }
}
```

## In-Game Remix

The Remix menu allows the player to open and reload the JSON without have to close the game.

## Remark

- Making the slugcat too tall might disjoint the body and head texture.
- Making the slugcat too small might make the hands appear really low.
- The slugcat seems to make more roll per roll when tinier. The roll distance however is unaffected.
