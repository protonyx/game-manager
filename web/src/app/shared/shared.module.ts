import {NgModule} from '@angular/core';
import {CustomMaterialModule} from "../custom-material/custom-material.module";
import {RouterModule} from "@angular/router";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {LayoutComponent} from "./layout/layout.component";


@NgModule({
    declarations: [
        LayoutComponent
    ],
    imports: [
        RouterModule,
        CustomMaterialModule,
        FormsModule,
        ReactiveFormsModule,
    ],
    exports: [
        CustomMaterialModule,
        FormsModule,
        ReactiveFormsModule,
        LayoutComponent
    ]
})
export class SharedModule {
}
