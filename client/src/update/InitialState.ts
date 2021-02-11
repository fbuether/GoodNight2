import type State from "../model/State";

import StartPage from "../model/StartPage";
import ReadPage from "../model/ReadPage";


export default function initialState(path: string): State {
  let page = path.startsWith("/read")
      ? new ReadPage()
      : new StartPage("default message!");

  return {
    page: page,
    user: "Mrs. Hollywookle"
  };
}
