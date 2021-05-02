
import Loading from "../../components/common/Loading";


export interface SignIn {
  page: "SignIn"
}


export function SignIn(state: SignIn) {
  return <Loading />;
}
