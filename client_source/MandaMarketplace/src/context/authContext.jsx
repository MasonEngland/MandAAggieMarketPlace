import { createContext } from "react";

const AuthContext = createContext({authenticated: false});
export default AuthContext;