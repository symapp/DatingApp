import { Component, OnInit } from '@angular/core';
import { AdminService } from "../../_services/admin.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photosForApproval: any;

  constructor(private adminService: AdminService, private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe(photos => {
      this.photosForApproval = photos;
      console.log(photos)
    })
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
      this.photosForApproval = this.photosForApproval.filter(photo => photo.id != photoId);
      this.toastr.info("Approved successfully")
    })
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe(() => {
      this.photosForApproval = this.photosForApproval.filter(photo => photo.id != photoId);
      this.toastr.info("Rejected successfully")
    })
  }

}
