import { Application, Router } from 'express';
import { IndexController } from '../controllers';

const router = Router();
const indexController = new IndexController();

export const setRoutes = (app: Application): void => {
    router.get('/sample-data', indexController.getSampleData.bind(indexController));
    app.use('/', router);
};