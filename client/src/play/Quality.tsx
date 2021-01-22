import * as React from "react";


export default interface Quality {
  name: string;
  icon: string;
  has: string;
}


export default function Quality(quality: Quality) {
  return (
    <li className="list-group-item d-flex">
      <div className="flex-fill">
        <div className="quality-icon-wrap">
          <img src={`assets/${quality.icon}`} className="quality-icon" />
        </div>
        {" "}{quality.name}
      </div>
      <span>{quality.has}</span>
    </li>
  );
}
