// import twocoins from "../../../assets/icons/two-coins.svg";
// import shamrock from "../../../assets/icons/shamrock.svg";
// import swapbag from "../../../assets/icons/swap-bag.svg";
// import sundial from "../../../assets/icons/sundial.svg";



// export type IconName =
//     | "shamrock"
//     | "two-coins"
//     | "swap-bag"
//     | "sundial";


// let icons: Record<IconName, string> = {
//   "shamrock": shamrock,
//   "two-coins": twocoins,
//   "swap-bag": swapbag,
//   "sundial": sundial
// };


const icons = {
  "shamrock": require("../../../assets/icons/shamrock.svg"),
  "two-coins": require("../../../assets/icons/two-coins.svg"),
  "swap-bag": require("../../../assets/icons/swap-bag.svg"),
  "sundial": require("../../../assets/icons/sundial.svg"),
  "save": require("../../../assets/icons/save.svg")
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
