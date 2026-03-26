import Navbar from './shared/components/Navbar';
import AppRoutes from './routes/AppRoutes';
import './App.css';

function App() {
  return (
    <div className="app">
      <Navbar />
      <main className="contenido-principal">
        <AppRoutes />
      </main>
    </div>
  );
}

export default App;
