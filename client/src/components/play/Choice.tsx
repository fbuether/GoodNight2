import Requirement from "../play/Requirement";


export default interface Choice {
  name: string;
  text: string;
  available: boolean;
  requirements: Array<Requirement>;
}

export default function Choice(choice: Choice) {
  let hasReqs = choice.requirements.length > 0;

  if (!hasReqs) {
    return (
      <a key={choice.name} className="list-group-item list-group-item-action" href="#">
        {choice.text}
      </a>
    );
  }

  let addReq = (rs: Array<JSX.Element>, r: Requirement) => {
    if (rs.length > 0) {
      rs.push(<span key={"empty"}>, </span>);
    }
    rs.push(<Requirement key={r.name} {...r}></Requirement>);
    return rs;
  };

  let requirements = choice.requirements.reduce(addReq,
    new Array<JSX.Element>());

  if (!choice.available) {
    return (
      <a key={choice.name} className="list-group-item list-group-item-action disabled">
        {choice.text} {" "}
        <small>(nicht verfügbar. benötigt: {requirements})</small>
      </a>
    );
  }

  return (
    <a key={choice.name} className="list-group-item list-group-item-action" href="#">
      {choice.text} {" "}
        <small>(benötigt: {requirements})</small>
    </a>
  );
}
