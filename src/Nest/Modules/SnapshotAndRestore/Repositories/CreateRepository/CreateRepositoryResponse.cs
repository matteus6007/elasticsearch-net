﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest
{
	public interface ICreateRepositoryResponse : IAcknowledgedResponse { }

	public class CreateRepositoryResponse : AcknowledgedResponse, ICreateRepositoryResponse { }
}
