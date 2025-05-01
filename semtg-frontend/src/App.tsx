import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import SearchPage from "./pages/Search/SearchPage"; // Adjust this import to your correct path
import TitleBar from "./components/TitleBar/TitleBar";
import CardDetail from "./pages/CardDetail/CardDetail";

function App() {
  return (
    <div id="root" className="app-container">
      <Router>
        <TitleBar />
        <Routes>
          <Route path="/search" element={<SearchPage />} />
          <Route path="/card/:id" element={<CardDetail />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
