import  FileBrowser  from "./pages/FileBrowser";
import Settings from "./pages/Settings";

const AppRoutes = [
  {
    path: '/home',
    element: <FileBrowser />
  },
  {
    path: '/settings',
    element: <Settings />
  }
];

export default AppRoutes;
