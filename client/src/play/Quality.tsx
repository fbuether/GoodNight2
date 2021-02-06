import * as React from "react";
import Icon, { Icons } from "../play/Icon";

export default interface Quality {
  name: string;
  icon: keyof typeof Icons;
  has: string;
}


export default function Quality(quality: Quality) {
  return (
    <li className="list-group-item d-flex">
      <div className="flex-fill">
        <Icon name={quality.icon} />
        {" "}{quality.name}
      </div>
      <span>{quality.has}</span>
    </li>
  );
}
