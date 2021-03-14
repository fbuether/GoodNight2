import twocoins from "../../../assets/icons/two-coins.svg";
import shamrock from "../../../assets/icons/shamrock.svg";
import swapbag from "../../../assets/icons/swap-bag.svg";
import sundial from "../../../assets/icons/sundial.svg";



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
  readonly class?: string;
}

export default function Icon(icon: Icon) {
  return (
    <span class={"icon " + (icon.class ?? "")}
      dangerouslySetInnerHTML={ { __html: icons[icon.name] } }></span>
  );
}
