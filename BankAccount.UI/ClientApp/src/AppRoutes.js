import { CreateAccount } from "./components/CreateAccount";
import { CreateAccountSuccess } from "./components/CreateAccountSuccess";
import { FetchData } from "./components/FetchData";
import { PotentialMember } from "./components/PotentialMember";
import { TermsAndConditions } from "./components/TermsAndConditions";

const AppRoutes = [
    {
        path: '/fetch-data',
        element: <FetchData />
    },
    {
        path: '/',
        element: <PotentialMember />
    },
    {
        path: '/terms',
        element: <TermsAndConditions />
    },
    {
        path: 'Create-account',
        element: <CreateAccount />
    },
    {
        path: 'success',
        element: <CreateAccountSuccess />
    }
];

export default AppRoutes;
