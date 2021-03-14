import twocoins from "../../../assets/icons/two-coins.svg";
import shamrock from "../../../assets/icons/shamrock.svg";
import swapbag from "../../../assets/icons/swap-bag.svg";
import sundial from "../../../assets/icons/swap-bag.svg";



export type IconName =
    | "shamrock"
    | "two-coins"
    | "swap-bag"
    | "sundial";


let icons: Record<IconName, string> = {
  "shamrock": shamrock,
  "two-coins": twocoins,
  "swap-bag": swapbag,
  "sundial": sundial
};


export function isIcon(name: string): name is IconName {
  return Object.keys(icons).includes(name);
}


export default interface Icon {
  readonly name: IconName;
}

export default function Icon(icon: Icon) {
  return (
    <span class="icon"
    dangerouslySetInnerHTML={ { __html: icons[icon.name] } }></span>
  );
}
