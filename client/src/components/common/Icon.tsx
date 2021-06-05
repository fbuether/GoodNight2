
const icons = {
  "astronaut-helmet": require("../../../assets/icons/astronaut-helmet.svg"),
  "bone-knife": require("../../../assets/icons/bone-knife.svg"),
  "bookmarklet": require("../../../assets/icons/bookmarklet.svg"),
  "crypt-entrance": require("../../../assets/icons/crypt-entrance.svg"),
  "embrassed-energy": require("../../../assets/icons/embrassed-energy.svg"),
  "empty-hourglass": require("../../../assets/icons/empty-hourglass.svg"),
  "flute": require("../../../assets/icons/flute.svg"),
  "forest": require("../../../assets/icons/forest.svg"),
  "ghost-ally": require("../../../assets/icons/ghost-ally.svg"),
  "hatchet": require("../../../assets/icons/hatchet.svg"),
  "horizon-road": require("../../../assets/icons/horizon-road.svg"),
  "ninja-head": require("../../../assets/icons/ninja-head.svg"),
  "saloon-doors": require("../../../assets/icons/saloon-doors.svg"),
  "save": require("../../../assets/icons/save.svg"),
  "secret-door": require("../../../assets/icons/secret-door.svg"),
  "shamrock": require("../../../assets/icons/shamrock.svg"),
  "shut-rose": require("../../../assets/icons/shut-rose.svg"),
  "sundial": require("../../../assets/icons/sundial.svg"),
  "swap-bag": require("../../../assets/icons/swap-bag.svg"),
  "two-coins": require("../../../assets/icons/two-coins.svg"),
} as const;


export type IconName = (keyof typeof icons);


export function isIcon(name: string): name is IconName {
  return Object.keys(icons).includes(name);
}


export default interface Icon {
  readonly name: IconName;
  readonly class?: string;
}

export default function Icon(icon: Icon) {
  let iconValue;
  if (Object.keys(icons).includes(icon.name)) {
    iconValue = icons[icon.name];
  }
  else {
    iconValue = "";
  }

  return (
    <span class={"icon " + (icon.class ?? "")}
      dangerouslySetInnerHTML={ { __html: iconValue } }></span>
  );
}
