import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Category } from '../../shared/models/category.model';
import { CategoryService } from '../../Core/services/category.service';


@Component({
  standalone: true,
  selector: 'app-category',
  imports: [CommonModule, FormsModule],
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css'],
})
export class CategoryComponent implements OnInit {
  categories: Category[] = [];
  categoryForm: Category = { name: '' };
  isEdit = false;

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.getAll().subscribe(data => 
      this.categories = data);
  }

  submitForm() {
    if (this.isEdit && this.categoryForm.categoryId) {
      this.categoryService.update(this.categoryForm).subscribe(() => {
        this.resetForm();
        this.fetchCategories();
      });
    } else {
      this.categoryService.add(this.categoryForm).subscribe(() => {
        this.resetForm();
        this.fetchCategories();
      });
    }
  }

  edit(category: Category) {
    this.categoryForm = { ...category };
    this.isEdit = true;
  }

  delete(id: number) {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.delete(id).subscribe(() => this.fetchCategories());
    }
  }

  resetForm() {
    this.categoryForm = { name: '' };
    this.isEdit = false;
  }
}
