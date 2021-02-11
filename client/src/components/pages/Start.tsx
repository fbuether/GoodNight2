
import StartPage from "../../model/StartPage";


export default function Start(state: { page: StartPage }) {
  return (
    <div>Hello! {state.page.message}</div>
  );
}
