import * as React from "react";
import Icon, { Icons } from "../play/Icon";

export default interface Quality {
  name: string;
  type: "bool" | "int" | "enum";
  icon: keyof typeof Icons;
  has: string;
}


export default function Quality(quality: Quality) {
  let text = <span>{quality.name}</span>;

  switch (quality.type) {
    case "int":
      text = <span>{quality.has} {quality.name}</span>;
      break;
    case "enum":
      text = <span>{quality.name} <small title={quality.has}>({quality.has})</small></span>;
      break;
  }

  return (
    <li className="list-group-item text-truncate">
      <Icon name={quality.icon} />{" "}{text}
    </li>
  );
}
