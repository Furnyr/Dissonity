import express from "express";
import path from "path";
import { fileURLToPath } from 'url';
import { dirname } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const app = express();

app.use(express.static(path.join(__dirname, "../../../")));

app.listen(3000, () => {
    console.log("Server listening on http://localhost:3000");
});

app.get("/", express.static(path.join(__dirname, "../../src/client")));