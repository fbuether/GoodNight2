import * as React from "react";

import twocoins from "../../assets/icons/two-coins.svg";
import shamrock from "../../assets/icons/shamrock.svg";


export type IconName =
    "shamrock"
    | "two-coins";


let icons: Record<IconName, string> = {
  "shamrock": shamrock,
  "two-coins": twocoins
};


export default interface Icon {
  readonly name: IconName;
}

export default function Icon(icon: Icon) {
  return (
    <span className="icon"
    dangerouslySetInnerHTML={ { __html: icons[icon.name] } }></span>
  );
}
