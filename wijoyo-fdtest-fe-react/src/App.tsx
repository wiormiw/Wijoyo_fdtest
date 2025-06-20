import {
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query';
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from 'react-router-dom';
import PublicBookFeed from './pages/PublicBookFeed';
import Login from './pages/Login';
import Register from './pages/Register';
import DashboardLayout from './components/DashboardLayout';
import Profile from './pages/Profile';
import Books from './pages/Books';
import Navbar from './components/Navbar';
import Users from './pages/Users';
import ForgotPassword from './pages/ForgotPassword';

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<PublicBookFeed />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/dashboard/*" element={<DashboardLayout />}>
            <Route path="books" element={<Books />} />
            <Route path="users" element={<Users />} />
          </Route>
          <Route path="/profile" element={<Profile />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
