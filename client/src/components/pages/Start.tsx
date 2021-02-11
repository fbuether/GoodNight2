
import StartPage from "../../model/StartPage";


export default function Start(state: StartPage) {
  return (
    <div>Hello! {state.message}</div>
  );
}
