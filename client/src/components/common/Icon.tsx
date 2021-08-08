
const icons = {
  "astronaut-helmet": require("../../../assets/icons/astronaut-helmet.svg"),
  "bone-knife": require("../../../assets/icons/bone-knife.svg"),
  "bookmarklet": require("../../../assets/icons/bookmarklet.svg"),
  "burning-passion": require("../../../assets/icons/burning-passion.svg"),
  "cog-lock": require("../../../assets/icons/cog-lock.svg"),
  "cook": require("../../../assets/icons/cook.svg"),
  "crypt-entrance": require("../../../assets/icons/crypt-entrance.svg"),
  "embrassed-energy": require("../../../assets/icons/embrassed-energy.svg"),
  "empty-hourglass": require("../../../assets/icons/empty-hourglass.svg"),
  "flute": require("../../../assets/icons/flute.svg"),
  "forest": require("../../../assets/icons/forest.svg"),
  "ghost-ally": require("../../../assets/icons/ghost-ally.svg"),
  "half-dead": require("../../../assets/icons/half-dead.svg"),
  "hatchet": require("../../../assets/icons/hatchet.svg"),
  "health-capsule": require("../../../assets/icons/health-capsule.svg"),
  "horizon-road": require("../../../assets/icons/horizon-road.svg"),
  "mine-explosion": require("../../../assets/icons/mine-explosion.svg"),
  "nested-hexagons": require("../../../assets/icons/nested-hexagons.svg"),
  "ninja-head": require("../../../assets/icons/ninja-head.svg"),
  "pianist": require("../../../assets/icons/pianist.svg"),
  "police-officer-head": require("../../../assets/icons/police-officer-head.svg"),
  "prisoner": require("../../../assets/icons/prisoner.svg"),
  "processor": require("../../../assets/icons/processor.svg"),
  "saloon-doors": require("../../../assets/icons/saloon-doors.svg"),
  "save": require("../../../assets/icons/save.svg"),
  "secret-door": require("../../../assets/icons/secret-door.svg"),
  "shamrock": require("../../../assets/icons/shamrock.svg"),
  "shop": require("../../../assets/icons/shop.svg"),
  "shut-rose": require("../../../assets/icons/shut-rose.svg"),
  "suitcase": require("../../../assets/icons/suitcase.svg"),
  "sundial": require("../../../assets/icons/sundial.svg"),
  "suspicious": require("../../../assets/icons/suspicious.svg"),
  "swap-bag": require("../../../assets/icons/swap-bag.svg"),
  "swipe-card": require("../../../assets/icons/swipe-card.svg"),
  "tombstone": require("../../../assets/icons/tombstone.svg"),
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
