
export default interface ActionLog {
  name: string;
  text: string;
}

export default function ActionLog(log: ActionLog) {
  return (
    <p className="action-log py-1">{log.text}</p>
  );
}
