import { Link, Outlet, useLocation } from 'react-router-dom';
import { useState } from 'react';

const navLinks = [
  { to: '/dashboard/books', label: 'Book Management' },
  { to: '/dashboard/users', label: 'Users' },
  { to: '/profile', label: 'Profile' },
];

export default function DashboardLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const location = useLocation();

  return (
    <div className="min-h-screen flex bg-gray-100">
      {/* Sidebar */}
      <aside className={`fixed inset-y-0 left-0 z-40 w-64 bg-white shadow-lg transform transition-transform duration-200 ease-in-out md:static md:translate-x-0 ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'} md:block`}>
        <div className="h-16 flex items-center px-6 font-bold text-indigo-700 text-xl border-b">Dashboard</div>
        <nav className="flex flex-col gap-2 p-4">
          {navLinks.map(link => (
            <Link
              key={link.to}
              to={link.to}
              className={`px-4 py-2 rounded font-medium transition ${location.pathname.startsWith(link.to) ? 'bg-indigo-100 text-indigo-700' : 'text-gray-700 hover:bg-gray-50'}`}
              onClick={() => setSidebarOpen(false)}
            >
              {link.label}
            </Link>
          ))}
        </nav>
      </aside>
      {/* Overlay for mobile */}
      {sidebarOpen && <div className="fixed inset-0 z-30 bg-black bg-opacity-30 md:hidden" onClick={() => setSidebarOpen(false)} />}
      {/* Main content */}
      <div className="flex-1 flex flex-col min-h-screen">
        {/* Topbar for mobile */}
        <header className="md:hidden flex items-center justify-between bg-white shadow px-4 h-16">
          <button onClick={() => setSidebarOpen(true)} className="p-2 rounded hover:bg-gray-100">
            <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" /></svg>
          </button>
          <span className="font-bold text-indigo-700 text-lg">Dashboard</span>
          <div />
        </header>
        <main className="flex-1 p-4 md:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
} 