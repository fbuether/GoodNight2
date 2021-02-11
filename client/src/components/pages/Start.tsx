import * as React from "react";

import StartPage from "../../model/StartPage.ts";


export default function Start(state: StartPage) {
  return (
    <div>Hello! {state.message}</div>
  );
}
