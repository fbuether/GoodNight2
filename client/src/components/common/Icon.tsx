

const icons = {
  "empty-hourglass": require("../../../assets/icons/empty-hourglass.svg"),
  "save": require("../../../assets/icons/save.svg"),
  "shamrock": require("../../../assets/icons/shamrock.svg"),
  "sundial": require("../../../assets/icons/sundial.svg"),
  "swap-bag": require("../../../assets/icons/swap-bag.svg"),
  "two-coins": require("../../../assets/icons/two-coins.svg"),
  "embrassed-energy": require("../../../assets/icons/embrassed-energy.svg"),
  "horizon-road": require("../../../assets/icons/horizon-road.svg"),
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
