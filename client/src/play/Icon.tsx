import * as React from "react";

import twocoins from "../../assets/icons/two-coins.svg";
import shamrock from "../../assets/icons/shamrock.svg";

export const Icons = {
  "shamrock": shamrock,
  "two-coins": twocoins
}


export default interface Icon {
  readonly name: keyof typeof Icons;
}

export default function Icon(icon: Icon) {
  return (
    <span className="icon"
    dangerouslySetInnerHTML={ { __html: Icons[icon.name] } }></span>
  );
}
