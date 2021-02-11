import Icon, {IconName} from "../play/Icon";


export default interface Requirement {
  name: string,
  icon: IconName,
  required: string,
  relation: string,
  has: string
}

export default function Requirement(state: Requirement) {

  return (
    <span key={state.name}>
      {state.relation} {state.required}/{state.has}
      <Icon name={state.icon} /> {state.name}
    </span>
  );
}
